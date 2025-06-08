import { createAsyncThunk } from "@reduxjs/toolkit";
// import axios from "axios";
import api from "../ApiConfig";

// const API_URL = "https://localhost:7152/api";

export const getListUser = createAsyncThunk(
  "user/getListUser",
  async (userId, { rejectWithValue }) => {
    try {
      const response = await api.get(`/user/getListUser?userId=${userId}`);
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);
