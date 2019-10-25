// <copyright file="webpack.config.js" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
const path = require("path");

module.exports = {
    entry: {
        index: "./src/index.js"
    },
    output: {
        path: path.resolve(__dirname, "../wwwroot/dist"),
        filename: "bundle.js"
    },
    module: {
        rules: [
            {
                use: {
                    loader: "babel-loader"
                },
                test: /\.js$/,
                exclude: /node_modules/ //excludes node_modules folder from being transpiled by babel. We do this because it's a waste of resources to do so.
            },
            {
                test: /\.(sa|sc|c)ss$/,
                use: ["style-loader", "css-loader"]
            }
        ]
    }
};