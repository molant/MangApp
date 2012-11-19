/**
 * User: molant
 * Date: 11/18/12
 * Time: 4:54 PM
 */
"use strict";
var providers = require('./providers/provider-loader.js'),
    mangaDb = require('./helpers/database.js'),
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}}",
        dateformat:"HH:MM:ss.L"
    });


if (process.argv[2] && process.argv[2] === "clean") {
    logger.log('Cleaning');
    mangaDb.clean();
    logger.log('DB - Updating');
    providers.update().then(function () {
        logger.log('DB - Updated');
    });
} else {
    logger.log('DB - Updating');
    providers.update(process.argv[2]).then(function () {
        logger.log('DB - Updated');
    });
}