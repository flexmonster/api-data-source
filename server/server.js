const express = require("express");
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
app.use(cors());
app.use(bodyParser.json());
app.use(express.static('public'))

app.use('/api/cube', require('./api/cube.js'));

app.listen(3400, () => {
    console.log("Server running on port 3400");
});