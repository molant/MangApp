/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:46 PM
 */
"use strict";

var fs = require('fs'),
    promised = require("promised-io/promise"),
    Deferred = require('promised-io').Deferred,
    providers = [];
//TODO: we shouldn't need lib in the path
fs.readdirSync('./providers/').forEach(function (file) {
    if (file.indexOf('-provider.js') !== -1) {
        providers.push(require('./' + file));
    }
});

function update() {
    var updaters = [];
    logger.log('updating DB');
    for (var i = 0; i < providers.length; i++) {
        updaters.push(providers[i].update());
    }
    return promised.all(updaters);
}

module.exports.providers = providers;
module.exports.update = update;