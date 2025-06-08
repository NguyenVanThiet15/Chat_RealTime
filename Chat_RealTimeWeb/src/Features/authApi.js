import { createAsyncThunk } from "@reduxjs/toolkit";
import api from "./ApiConfig";

export const loginUser = createAsyncThunk(
  "auth/login",
  async (values, { rejectWithValue }) => {
    try {
      const response = await api.post(`/user/login`, values);
      // localStorage.setItem("token", response.data.token);
      // localStorage.setItem(
      //   "user",
      //   JSON.stringify({
      //     id: response.data.userId,
      //     email: response.data.email,
      //     userName: response.data.userName,
      //   })
      // );
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
      const response = await api.post(`/user/register`, values);
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);
export const loginByGoogle = createAsyncThunk(
  "auth/loginbygoogle",
  async ({ TokenGoogle: TokenGoogle }, { rejectWithValue }) => {
    try {
      const response = await api.post(`/user/loginByGoogle`, {
        TokenGoogle: TokenGoogle,
      });
      return response.data;
    } catch (err) {
      return rejectWithValue(err.response.data);
    }
  }
);
