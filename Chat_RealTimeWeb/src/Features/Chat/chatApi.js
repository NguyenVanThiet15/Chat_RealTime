import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_URL = "https://localhost:7152/api";

export const createOrGetChat = createAsyncThunk(
  "chat/createOrGetChat",
  async ({ currentUserId, targetUserId }, { rejectWithValue }) => {
    try {
      debugger;
      const response = await axios.post(`${API_URL}/chat/createOrGetChat`, {
        currentUserId,
        targetUserId,
      });
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
// export const sendMessage = createAsyncThunk(
//   "chat/sendMessage",
//   async ({ currentUserId, chatId, targetUserId }, { rejectWithValue }) => {
//     try {
//       debugger;
//       const response = await axios.post(
//         `${API_URL}/chat/${chatId}/senMessage`,
//         {
//           currentUserId,
//           targetUserId,
//         }
//       );
//       return response.data;
//     } catch (error) {
//       return rejectWithValue(error.message);
//     }
//   }
// );
export const getMessage = createAsyncThunk(
  "chat/senMessage",
  async ({ chatId }, { rejectWithValue }) => {
    try {
      const response = await axios.get(`${API_URL}/chat/${chatId}/getMessage`);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
export const sendMessageImage = createAsyncThunk(
  "chat/sendMessage",
  async ({ chatId, imageFile, senderId }, { rejectWithValue }) => {
    try {
      debugger;
      debugger;
      const response = await axios.post(
        `${API_URL}/chat/${chatId}/SendMessageImg`,
        {
          imageFile,
          senderId,
        }
      );
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
