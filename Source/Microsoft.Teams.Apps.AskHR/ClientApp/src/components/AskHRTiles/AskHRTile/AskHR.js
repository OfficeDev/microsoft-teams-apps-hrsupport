import React, { Component } from 'react';
import './AskHR.css';
import { withAITracking } from '@microsoft/applicationinsights-react-js';
import { ai } from '../../Telemetry/TrackTelemetry';

class AskHR extends Component {

    render() {
        return (
            <React.Fragment>
                <a onClick={this.props.handleTelemetry} href={this.props.AskHRData.redirectUrl} aria-label={"open " + this.props.AskHRData.title + " tile"} target="_blank">
                        <div className="tile">
                            <div className="title">
                                {this.props.AskHRData.title}
                            </div>
                        <div className="imageAskHRDiv">
                            <img title={this.props.AskHRData.title} className="imageAskHR" src={this.props.AskHRData.imageUrl} alt={this.props.AskHRData.title} />
                            </div>
                            <div className="descDiv">
                                <div title={this.props.AskHRData.description} className="description">
                                     {this.props.AskHRData.description}
                                </div>
                            </div>
                        </div>
                    </a> 
            </React.Fragment>
         );
    }
}
 
export default withAITracking(ai.reactPlugin, AskHR, "AskHR");