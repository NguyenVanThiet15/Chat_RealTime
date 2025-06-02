import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_URL = "https://localhost:7152/api";

export const loginUser = createAsyncThunk(
  "auth/login",
  async (values, { rejectWithValue }) => {
    try {
      debugger;
      const response = await axios.post(`${API_URL}/user/login`, values);
      localStorage.setItem("token", response.data.token);
      localStorage.setItem(
        "user",
        JSON.stringify({
          id: response.data.userId,
          email: response.data.email,
          userName: response.data.userName,
        })
      );
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);

export const registerUser = createAsyncThunk(
  "auth/registerUser",
  async (values, { rejectWithValue }) => {
    try {
      const response = await axios.post(`${API_URL}/user/register`, values);
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);
export const loginByGoogle = createAsyncThunk(
  "auth/loginbygoogle",
  async ({ TokenGoogle : TokenGoogle }, { rejectWithValue }) => {
    try {
      const response = await axios.post(`${API_URL}/user/loginByGoogle`, {
        TokenGoogle: TokenGoogle,
      });
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);
