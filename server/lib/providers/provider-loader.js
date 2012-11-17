/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:46 PM
 */
"use strict";

var fs = require('fs'),
    providers = [];
//TODO: we shouldn't need lib in the path
fs.readdirSync('./providers/').forEach(function (file) {
    if (file.indexOf('-provider.js') !== -1) {
        providers.push(require('./'+ file));
    }
});

module.exports.providers = providers;