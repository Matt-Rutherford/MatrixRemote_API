import axios from "axios";
import { MatrixEvent } from "../models/MatrixEvent";

// Set the base URL for your API
const api = axios.create({
  baseURL: "http://192.168.x.x:7033/api/RemoteAPI", // eventually replace with backend url
});

export const getEvents = async (): Promise<MatrixEvent[]> => {
  const response = await api.get<MatrixEvent[]>("/GetEvents");
  return response.data;
};

export const createEvent = async (event: MatrixEvent): Promise<MatrixEvent> => {
  const response = await api.post<MatrixEvent>("/CreateEvent", event);
  return response.data;
};

export const deleteEvent = async (id: string): Promise<void> => {
  await api.delete(`/DeleteMessage/${id}`);
};
