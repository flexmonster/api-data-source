const express = require("express");
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
app.use(cors());
app.use(bodyParser.json());

app.use('/api/cube', require('./api/cube.js'));

const port = process.argv[2] || 3400;
app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});