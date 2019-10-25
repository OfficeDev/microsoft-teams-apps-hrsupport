import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import React, { Component } from 'react';

class TelemetryService extends Component {

    constructor(props) {
        super(props);
        this.reactPlugin = new ReactPlugin();
    }

    getKey(reactPluginConfig) {
        fetch('/Help/GetConfigurationJson')
            .then(response => response.json())
            .then(data => {
                if (data && data.APPINSIGHTS_INSTRUMENTATIONKEY) {
                    this.loadAppInsights(data.APPINSIGHTS_INSTRUMENTATIONKEY, reactPluginConfig);
                }
            },
                (error) => {
                    console.error(error);
                    ai.appInsights.trackException({ name: error });
                });
    }

    initialize(reactPluginConfig) {
        this.getKey(reactPluginConfig);
    }

    loadAppInsights(instrumentationKey, reactPluginConfig) {
        this.appInsights = new ApplicationInsights({
            config: {
                instrumentationKey: instrumentationKey,
                maxBatchInterval: 0,
                disableFetchTracking: false,
                extensions: [this.reactPlugin],
                extensionConfig: {
                    [this.reactPlugin.identifier]: reactPluginConfig
                }
            }
        });
        this.appInsights.loadAppInsights();
    }
}
export let ai = new TelemetryService();