var callback = arguments[arguments.length - 1];

var context = typeof arguments[0] === 'string' ? JSON.parse(arguments[0]) : arguments[0];
context = context || document;

var options = JSON.parse(arguments[1]);

axe.run(context, options, function (err, res) {
    {  
        if (err) {
            callback({ error: err.message }, res);
        }
        else {
            callback(res);
        }
    }
});