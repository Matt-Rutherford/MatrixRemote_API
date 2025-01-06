import React from "react";
import MapComponent from "./components/MapComponent";
import Chatbox from "./components/Chatbox";
import "./App.css";

const App: React.FC = () => {
    return (
        <div className="app-container">
            {/* Header */}
            <header className="header">MatrixRemote</header>

            {/* Main Content: Map and Chatbox */}
            <div className="main-content">
                <div className="map-container">
                    <MapComponent />
                </div>
                <div className="chatbox-container">
                    <Chatbox />
                </div>
            </div>
        </div>
    );
};

export default App;
