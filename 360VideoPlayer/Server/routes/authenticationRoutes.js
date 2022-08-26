const mongoose = require('mongoose');
const Account = mongoose.model('accounts');

module.exports = app => {
    app.get('/account',async(req, res) => {

        const { rUsername, rPassword } = req.query;
        if(rUsername == null || rPassword == null){
            res.send("Invalid credentials");
            return;
        }
        
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount == null){
            console.log("Account not exists");
            res.send("Account not exists");
            return;
        }else{
            if(rPassword == userAccount.password){
                userAccount.lastAuthentication = Date.now();
                await userAccount.save();
                
                console.log("retrieving account");
                res.send(userAccount);
                return;
            }
        }

        res.send("Invalid credentials");
        return;
        
    });

    app.get('/register',async(req, res) => {

        const { rUsername, rPassword } = req.query;
        
        if(rUsername == null || rPassword == null){
            res.send("Invalid credentials");
            return;
        }
        
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount == null){
            console.log("create new account");

            var newDriveIDArray = new Account({
                username : rUsername,
                password : rPassword,
                googledriveVideoID : '{"videoID":[]}',
                googledrive360VideoID : '{"360videoID":[]}',
                googledriveImageID : '{"imageID":[]}',
                youtubeVideoUrl : '{"youtubeVideoUrl":[]}',
                youtube360VideoUrl : '{"youtube360VideoUrl":[]}',

                lastAuthentication : Date.now()
            })
            await newDriveIDArray.save();

            res.send(newDriveIDArray);
            return;
        }else{
            if(rUsername == userAccount.username){
                
                res.send("Account exists");
                return;
            }
        }

        res.send("Invalid credentials");
        return;
    });
    

    app.get('/updateDriveVideoID',async(req, res) => {

        const {rUsername,rGoogledriveVideoID} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            await Account.findOneAndUpdate({username:rUsername},{googledriveVideoID:rGoogledriveVideoID},{new: true}).then((data) => {
                console.log('data', data)
              })
            console.log("uploadID");
            res.send("update success");
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/updateDrive360VideoID',async(req, res) => {

        const {rUsername,rGoogledrive360VideoID} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            await Account.findOneAndUpdate({username:rUsername},{googledrive360VideoID:rGoogledrive360VideoID},{new: true}).then((data) => {
                console.log('data', data)
              })
            console.log("uploadID");
            res.send("update success");
            return;
        }

        res.send("Invalid credentials");
        return;
    });
    
    app.get('/updateDriveImageID',async(req, res) => {

        const {rUsername,rGoogledriveImageID} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            await Account.findOneAndUpdate({username:rUsername},{googledriveImageID:rGoogledriveImageID},{new: true}).then((data) => {
                console.log('data', data)
              })
            console.log("uploadID");
            res.send("update success");
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/updateYoutubeVideoUrl',async(req, res) => {

        const {rUsername,rYoutubeVideoUrl} = req.query;
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            await Account.findOneAndUpdate({username:rUsername},{youtubeVideoUrl:rYoutubeVideoUrl},{new: true}).then((data) => {
                console.log('data', data)
              })
            console.log("uploadUrl");
            res.send("update success");
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/updateYoutube360VideoUrl',async(req, res) => {

        const {rUsername,rYoutube360VideoUrl} = req.query;
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            await Account.findOneAndUpdate({username:rUsername},{youtube360VideoUrl:rYoutube360VideoUrl},{new: true}).then((data) => {
                console.log('data', data)
              })
            console.log("uploadUrl");
            res.send("update success");
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/getDriveVideoID',async(req, res) => {

        const {rUsername} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            console.log(userAccount.googledriveVideoID);
            res.send(userAccount.googledriveVideoID);
            return;
        }

        res.send("Invalid credentials");
        return;
    });
    
    app.get('/getDrive360VideoID',async(req, res) => {

        const {rUsername} = req.query;
        //console.log(rgoogledrive360VideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            console.log(userAccount.googledrive360VideoID);
            res.send(userAccount.googledrive360VideoID);
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/getDriveImageID',async(req, res) => {

        const {rUsername} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            console.log(userAccount.googledriveImageID);
            res.send(userAccount.googledriveImageID);
            return;
        }
        
        res.send("Invalid credentials");
        return;
    });

    app.get('/getYoutubeVideoUrl',async(req, res) => {

        const {rUsername} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            console.log(userAccount.youtubeVideoUrl);
            res.send(userAccount.youtubeVideoUrl);
            return;
        }

        res.send("Invalid credentials");
        return;
    });

    app.get('/getYoutube360VideoUrl',async(req, res) => {

        const {rUsername} = req.query;
        //console.log(rGoogledriveVideoID);
        var userAccount = await Account.findOne({username: rUsername});
        if(userAccount != null){
            console.log(userAccount.youtube360VideoUrl);
            res.send(userAccount.youtube360VideoUrl);
            return;
        }

        res.send("Invalid credentials");
        return;
    });
}
