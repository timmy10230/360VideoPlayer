const mongoose = require('mongoose');
const { Schema } = mongoose;

const accountSchema = new Schema({
    username: String,
    password: String,
    googledriveVideoID: String,
    googledrive360VideoID: String,
    googledriveImageID: String,
    youtubeVideoUrl: String,
    youtube360VideoUrl: String,
    
    lastAuthentication: Date,
})

mongoose.model('accounts',accountSchema);