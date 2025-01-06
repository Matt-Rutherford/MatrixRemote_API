
export interface GeoLocation {
    latitude: number;
    longitude: number;
  }
  
  export enum EventType {
    Text = "Text",
    Image = "Image",
    GIF = "GIF",
  }
  
  export interface RgbColor {
    r: number;
    g: number;
    b: number;
  }
  
  export interface MatrixEvent {
    id: string;
    content: string;
    timestamp: string; // Use ISO 8601 string
    location: GeoLocation;
    type: EventType;
    color?: RgbColor; // Optional for non-text events
  }
  