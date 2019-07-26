const cube = require('express').Router();
const fs = require('fs').promises;
const _ = require('lodash');

/**
 * API endpoint
 */
cube.post("/", async (req, res) => {
    try {
        let result = null;
        if (req.body.type == "fields") {
            result = await getSchema(req.body.index);
        } else if (req.body.type == "members") {
            result = await getMembers(req.body.index, req.body.field, req.body.page);
        } else if (req.body.type == "select") {
            result = await getSelectResult(req.body.index, req.body.query, req.body.page);
        } else {
            throw `Request type '${req.body.type}' is not implemented.`;
        }
        res.json(result);
    } catch (err) {
        handleError(err, res);
    }
});

/**
 * ==============
 * FIELDS REQUEST
 * ==============
 */

/**
 * Gets index schema. Reads the `_schema.json` file from the `./data` folder.
 * @param {string} index index name
 */
async function getSchema(index) {
    if (!fieldsCache[index]) {
        const fields = await fs.readFile(`./data/${index}_schema.json`);
        fieldsCache[index] = JSON.parse(fields);
    }
    return fieldsCache[index];
}
const fieldsCache = {};

/**
 * Gets field info from the schema.
 * @param {string} index index name
 * @param {string} field field's name
 */
async function getField(index, field) {
    const schema = await getSchema(index);
    return _.find(schema.fields, f => f.field == field);
}

/**
 * ===============
 * MEMBERS REQUEST
 * ===============
 */

const MEMBERS_PAGE_SIZE = 5000;

/**
 * Gets field members.
 * @param {string} index index name
 * @param {string} fieldName field's name
 * @param {number} page page number to load
 */
async function getMembers(index, fieldName, page) {
    const data = await getData(index);
    const field = await getField(index, fieldName);
    const output = {
        members: []
    };
    const members = _.uniq(_.map(data, fieldName));

    page = isNaN(page) ? 0 : page;
    const pageTotal = Math.ceil(members.length / MEMBERS_PAGE_SIZE);
    if (pageTotal > 1) {
        output.page = page;
        output.pageTotal = Math.ceil(members.length / MEMBERS_PAGE_SIZE);
    }
    const from = page * MEMBERS_PAGE_SIZE;
    const size = Math.min(members.length, from + MEMBERS_PAGE_SIZE);
    for (let i = from; i < size; i++) {
        output.members.push(createMember(members[i], field.type));
    }
    return output;
}

function createMember(value, fieldType) {
    return {
        value: value
    };
}

/**
 * ==============
 * SELECT REQUEST
 * ==============
 */

const SELECT_PAGE_SIZE = 5000;

/**
 * Gets select data.
 * @param {string} index index name
 * @param {object} query query object
 * @param {number} page page number
 */
async function getSelectResult(index, query, page) {
    const output = {};
    let data = await getData(index);
    if (query.filter) {
        data = filterData(data, query.filter);
    }
    if (query.aggs && query.aggs.values) {
        output.aggs = [];
        if (query.aggs.by) {
            const rows = query.aggs.by.rows || [];
            const cols = query.aggs.by.cols || [];
            calcByFields(data, rows.concat(cols), cols, query.aggs.values, output.aggs); // data
            output.aggs.push(calcValues(data, query.aggs.values)); // grand totals
        } else { // only grand totals
            output.aggs.push(calcValues(data, query.aggs.values));
        }
        page = isNaN(page) ? 0 : page;
        const pageTotal = Math.ceil(output.aggs.length / SELECT_PAGE_SIZE);
        if (pageTotal > 1) {
            output.page = page;
            output.pageTotal = Math.ceil(output.aggs.length / SELECT_PAGE_SIZE);
            const from = page * SELECT_PAGE_SIZE;
            const size = Math.min(output.aggs.length, from + SELECT_PAGE_SIZE);
            output.aggs = output.aggs.slice(from, size);
        }
    }
    if (query.fields) {
        output.fields = query.fields;
        output.hits = [];
        const limit = isNaN(query.limit) ? data.length : Math.min(query.limit, data.length);
        for (let i = 0; i < limit; i++) {
            const row = query.fields.map(f => data[i][f.field]);
            output.hits.push(row)
        }
        page = isNaN(page) ? 0 : page;
        const pageTotal = Math.ceil(output.hits.length / SELECT_PAGE_SIZE);
        if (pageTotal > 1) {
            output.page = page;
            output.pageTotal = Math.ceil(output.hits.length / SELECT_PAGE_SIZE);
            const from = page * SELECT_PAGE_SIZE;
            const size = Math.min(output.hits.length, from + SELECT_PAGE_SIZE);
            output.hits = output.hits.slice(from, size);
        }
    }
    return output;
}

/**
 * Filters data.
 * @param {Array} data input data 
 * @param {Array} filters filters to apply
 */
function filterData(data, filters) {
    if (filters.length == 0) {
        return data;
    }
    return _.filter(data, d => {
        for (const filter of filters) {
            const check = checkFilter(d, filter);
            if (!check) {
                return false;
            }
        }
        return true;
    });
}

/**
 * Checks whether the data item meets the filter query.
 * @param {object} item data item
 * @param {object} filter filter object 
 */
function checkFilter(item, filter) {
    let check = true;
    const field = filter["field"];
    const value = item[field];
    if (filter["include"]) {
        check = filter["include"].indexOf(value) >= 0;
    } else if (filter["exclude"]) {
        check = filter["exclude"].indexOf(value) < 0;
    } else if (filter["query"]) {
        const query = filter["query"];
        if (filter["fieldType"] == "date") {
            check = checkDateFilterQuery(value, query);
        } else if (filter["fieldType"] == "number") {
            check = checkNumberFilterQuery(value, query);
        } else {
            check = checkStringFilterQuery(value, query);
        }
    }
    return check;
}

const MS_DAY = 24 * 60 * 60 * 1000;

/**
 * Checks whether the timestamp meets the query condition.
 * @param {number} value Unix timestamp
 * @param {object} query query object
 */
function checkDateFilterQuery(value, query) {
    if (query["equal"] !== undefined) {
        var d = query["equal"];
        return value - d >= 0 && value - d < MS_DAY;
    }
    if (query["not_equal"] !== undefined) {
        var d = query["not_equal"];
        return value < d || value >= d + MS_DAY;
    }
    if (query["after"] !== undefined) {
        var d = query["after"];
        return value >= d + MS_DAY;
    }
    if (query["after_equal"] !== undefined) {
        var d = query["after_equal"];
        return value >= d;
    }
    if (query["before"] !== undefined) {
        var d = query["before"];
        return value < d;
    }
    if (query["before_equal"] !== undefined) {
        var d = query["before_equal"];
        return value < d + MS_DAY;
    }
    if (query["between"] !== undefined) {
        var d1 = query["between"][0];
        var d2 = query["between"][1];
        return value >= d1 && value < d2 + MS_DAY;
    }
    if (query["not_between"] !== undefined) {
        var d1 = query["not_between"][0];
        var d2 = query["not_between"][1];
        return value < d1 || value >= d2 + MS_DAY;
    }
    return false;
}

/**
 * Checks whether the string value meets the query condition.
 * @param {string} value string value
 * @param {object} query query object
 */
function checkStringFilterQuery(value, query) {
    value = String(value).toLowerCase();
    if (query["equal"] !== undefined) {
        return value == String(query["equal"]).toLowerCase();
    }
    if (query["not_equal"] !== undefined) {
        return value != String(query["not_equal"]).toLowerCase();
    }
    if (query["begin"] !== undefined) {
        return value.indexOf(String(query["begin"]).toLowerCase()) == 0;
    }
    if (query["not_begin"] !== undefined) {
        return value.indexOf(String(query["not_begin"]).toLowerCase()) != 0;
    }
    if (query["end"] !== undefined) {
        const idx = value.lastIndexOf(String(query["end"]).toLowerCase())
        return idx >= 0 && idx + query["end"].length == value.length;
    }
    if (query["not_end"] !== undefined) {
        const idx = value.lastIndexOf(String(query["not_end"]).toLowerCase())
        return idx < 0 || idx + query["not_end"].length != value.length;
    }
    if (query["contain"] !== undefined) {
        const idx = value.indexOf(String(query["contain"]).toLowerCase())
        return idx >= 0;
    }
    if (query["not_contain"] !== undefined) {
        const idx = value.indexOf(String(query["not_contain"]).toLowerCase())
        return idx < 0;
    }
    if (query["greater"] !== undefined) {
        return value > String(query["greater"]).toLowerCase();
    }
    if (query["greater_equal"] !== undefined) {
        return value >= String(query["greater_equal"]).toLowerCase();
    }
    if (query["less"] !== undefined) {
        return value < String(query["less"]).toLowerCase();
    }
    if (query["less_equal"] !== undefined) {
        return value <= String(query["less_equal"]).toLowerCase();
    }
    if (query["between"] !== undefined) {
        var v1 = String(query["between"][0]).toLowerCase();
        var v2 = String(query["between"][1]).toLowerCase();
        return value >= v1 && value <= v2;
    }
    if (query["not_between"] !== undefined) {
        var v1 = String(query["not_between"][0]).toLowerCase();
        var v2 = String(query["not_between"][1]).toLowerCase()
        return value < v1 || value > v2;
    }
    return false;
}

/**
 * Checks whether the numeric value meets the query condition.
 * @param {number} value numeric value
 * @param {object} query query object
 */
function checkNumberFilterQuery(value, query) {
    if (query["equal"] !== undefined) {
        return value == query["equal"];
    }
    if (query["not_equal"] !== undefined) {
        return value != query["not_equal"];
    }
    if (query["greater"] !== undefined) {
        return value > query["greater"];
    }
    if (query["greater_equal"] !== undefined) {
        return value >= query["greater_equal"];
    }
    if (query["less"] !== undefined) {
        return value < query["less"];
    }
    if (query["less_equal"] !== undefined) {
        return value <= query["less_equal"];
    }
    if (query["between"] !== undefined) {
        return value >= query["between"][0] && value <= query["between"][1];
    }
    if (query["not_between"] !== undefined) {
        return value < query["not_between"][0] || value > query["not_between"][1];
    }
    return false;
}

/**
 * Groups data by fields and calculates numberic data. Works recursively.
 * @param {object[]} data input data
 * @param {string[]} fields all fields to group by
 * @param {string[]} cols fields in columns to group by
 * @param {object[]} values values to calcluate
 * @param {object[]} output output response
 * @param {object} keys key-value pairs that describes specific tuple
 */
function calcByFields(data, fields, cols, values, output, keys) {
    if (fields.length < 1) {
        return;
    }
    const field = fields[0];
    const subfields = fields.slice(1);
    const groups = _.groupBy(data, field);
    for (const key in groups) {
        const subdata = groups[key];
        const item = calcValues(subdata, values);
        item.keys = keys ? _.clone(keys) : {};
        item.keys[field] = key;
        output.push(item);
        calcByFields(subdata, subfields, cols, values, output, item.keys);
    }
    // column totals
    if (cols && cols.length > 0 && fields.length > cols.length) {
        const colField = cols[0];
        const subCols = cols.slice(1);
        const colGroups = _.groupBy(data, colField);
        for (const key in colGroups) {
            const subdata = colGroups[key];
            const item = calcValues(subdata, values);
            item.keys = keys ? _.clone(keys) : {};
            item.keys[colField] = key;
            output.push(item);
            calcByFields(subdata, subCols, null, values, output, item.keys);
        }
    }
}

/**
 * Calculates aggregated values.
 * @param {object[]} data input data 
 * @param {object} values values to calculate
 */
function calcValues(data, values) {
    const output = {
        values: {}
    };
    for (const value of values) {
        if (!output.values[value.field]) {
            output.values[value.field] = {};
        }
        output.values[value.field][value.func] = calcValue(data, value.field, value.func);
    }
    return output;
}

/**
 * Calculates aggregated value for specific field.
 * @param {object} data input data
 * @param {string} field field's name
 * @param {string} func aggregation name
 */
function calcValue(data, field, func) {
    if (func == "sum") {
        return _.sumBy(data, field);
    }
    if (func == "count") {
        return data.length;
    }
    if (func == "distinctcount") {
        return _.uniqBy(data, field).length;
    }
    if (func == "average") {
        return _.sumBy(data, field) / data.length;
    }
    if (func == "min") {
        return _.minBy(data, field);
    }
    if (func == "max") {
        return _.maxBy(data, field);
    }
    return NaN;
}

/**
 * Gets index raw data. Reads the `.json` file from the `./data` folder. 
 * @param {string} index index name
 */
async function getData(index) {
    if (!dataCache[index]) {
        const dataRaw = await fs.readFile(`./data/${index}.json`);
        const data = JSON.parse(dataRaw);
        dataCache[index] = data;
    }
    return dataCache[index];
}
const dataCache = {};

function handleError(err, res, status) {
    if (!res) {
        throw "Second parameter is required";
    }
    console.error(err);
    status = status || 500;
    var message = "Unknown server error.";
    if (typeof err == "string") {
        message = err;
    } else if (err.message) {
        message = err.message;
    }
    res.status(status).json({
        message
    });
}

module.exports = cube;