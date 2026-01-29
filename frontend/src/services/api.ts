import axios from "axios";

export const api = axios.create({
  baseURL: "https://localhost:7286/api",
  headers: {
    "Content-Type": "application/json",
  },
});
