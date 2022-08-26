const express = require('express');
const keys = require('./config/keys.js')

const app = express();

const mongoose = require('mongoose');
mongoose.connect(keys.mongoURI, {useNewUrlParser: true, useUnifiedTopology: true});

require('./model/Account');

require('./routes/authenticationRoutes')(app);

app.listen(keys.port, ()=>{
    console.log("listen on port:" + keys.port);
});