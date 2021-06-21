import React, { Component } from 'react';
import AskHRTile from './AskHRTile/AskHR';
import './AskHRTiles.css';
import * as microsoftTeams from '@microsoft/teams-js';
import { createBrowserHistory } from "history";
import { ai } from '../Telemetry/TrackTelemetry';
import i18next from 'i18next';

//contains the record for click event of tile
const history = createBrowserHistory({ basename: '' });
ai.initialize({ history: history });

class AskHRTiles extends Component {
    state = {
        askHRTabData: [],
        hasData: false,
        noDataAvailableMessage: "",
        error: null,
        isCultureRightToLeft: false
    }

    componentWillMount() {
        if (this.state.askHRTabData.length > 0) {
            this.setState({ hasData: true });
        }
        else {
            this.setState({ hasData: false });
        }

        //// Call the initialize API first
        microsoftTeams.initialize();

        //// Check the initial theme and locale user chose and respect it
        microsoftTeams.getContext((context) => {
            if (context && context.theme) {
                this.setTheme(context.theme);
                this.setCultureDirection(context.locale);
            }
        });

        // Handle theme changes
        microsoftTeams.registerOnThemeChangeHandler((theme) => {
            this.setTheme(theme);
        });
    }

    componentDidMount() {
        let token = localStorage.getItem("adal.idtoken");
        if (token) {
            fetch('/Help/GetHelpTiles', {
                method: "post",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token
                }
            }).then(response => response.json())
                .then(data => {
                    if (data.entities.length !== undefined) {
                        if (data.entities.length > 0) {
                            this.setState({
                                noDataAvailableMessage: "",
                                askHRTabData: data.entities
                            });
                            document.getElementById("root").style.display = "block";
                        }
                        else {
                            this.setState({ noDataAvailableMessage: "My Team of Experts hasn't added anything here yet."});
                            console.log(this.state.noDataAvailableMessage);
                        }
                    }
                    else {
                        this.setState({ noDataAvailableMessage: "Oops!!! Something went wrong." });
                        console.log(this.state.noDataAvailableMessage);
                    }
                },
                    (error) => {
                        this.setState({ error: error, noDataAvailableMessage: "Oops!!! Something went wrong." });
                        console.error(this.state.error);
                        ai.appInsights.trackException({ name: this.state.error });
                    })
                .catch((error) => {
                    this.setState({ error: error, noDataAvailableMessage: "Oops!!! Something went wrong." });
                    console.error(this.state.error);
                    ai.appInsights.trackException({ name: this.state.error });
                });
        }
        else {
            console.log("token is found null in cache. User is required to explicitly sign in.");
        }
    }

    /**
     * Set the culture direction according to given locale
     * @param {any} locale Selected locale.
     */
    setCultureDirection(locale) {
        i18next.init({ lng: locale });
        var isRtl = i18next.dir(locale) === "rtl";
        this.setState({ isCultureRightToLeft: isRtl });
    }

    // Set the desired theme
    setTheme(theme) {
        if (theme) {
            // Possible values for theme: 'default', 'light', 'dark' and 'contrast'
            document.body.className = 'theme' + '-' + (theme === "default" ? "light" : theme);
        }
    }

    trackEventAskHR() {
        console.log('Ask HR Opening New Page');
        ai.appInsights.trackEvent({ name: 'Ask HR Opening New Page' });
    }

    render() {
        var num = 0;
        const data = [...this.state.askHRTabData];
        let askHRDataTile = null;
        if (this.state.askHRTabData.length > 0) {
            askHRDataTile = data.
                map(askHRData => {
                    num++;
                    return <AskHRTile handleTelemetry={this.trackEventAskHR} key={num} AskHRData={askHRData} cultureInfo={this.state.isCultureRightToLeft} />;
                });
        }
        else {
            askHRDataTile = <div id="noDataAvailableMsg" aria-live="polite" className="noDataDiv"> {this.state.noDataAvailableMessage}</div>;
        }
        return (
            <div className={this.state.isCultureRightToLeft ? "rtlAlign" : "ltrAlign"}>
                {askHRDataTile}
            </div>
        );
    }
}

export default AskHRTiles;
