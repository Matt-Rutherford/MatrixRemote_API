import React from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";

const MapComponent: React.FC = () => {
    return (
        <MapContainer
            center={[37.0902, -95.7129]} // Center of the USA
            zoom={4} // Ensure this is a number, not a bigint
            style={{ height: "100%", width: "100%" }} // Full height and width
        >
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            />
            <Marker position={[37.0902, -95.7129]}>
                <Popup>Example Message</Popup>
            </Marker>
        </MapContainer>
    );
};

export default MapComponent;

