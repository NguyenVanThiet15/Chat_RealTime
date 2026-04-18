import { createAsyncThunk } from "@reduxjs/toolkit";

import api from "../ApiConfig";

// const API_URL = "https://localhost:7152/api";

export const createOrGetChat = createAsyncThunk(
  "chat/createOrGetChat",
  async (
    { nameRoom, roomId, nguoiGuiId, nguoiNhanId, participant, chatType },
    { rejectWithValue }
  ) => {
    try {
      debugger;
      const response = await api.post(`/chat/createOrGetChat`, {
        nguoiGuiId,
        nguoiNhanId,
        nameRoom,
        roomId,
        participant,
        chatType,
      });
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
// export const sendMessageImg = createAsyncThunk(
//   "chat/sendMessageImg",
//   async ({ currentUserId, chatId, targetUserId }, { rejectWithValue }) => {
//     try {
//       debugger;
//       const response = await api.post(`/chat/${chatId}/senMessage`, {
//         currentUserId,
//         targetUserId,
//       });
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
      const response = await api.get(`/chat/${chatId}/getMessage`);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
export const sendMessageImage = createAsyncThunk(
  "chat/sendMessageImg",
  async ({ chatId, file, senderId }, { rejectWithValue }) => {
    try {
      debugger;
      const formData = new FormData();
      formData.append("file", file);
      formData.append("senderId", senderId);
      const response = await api.post(
        `/chat/${chatId}/getMessageImage`,
        formData,
        {
          headers: {
            // Authorization: `Bearer ${token}`,
            "Content-Type": "multipart/form-data", // Quan trọng!
          },
        }
      );
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
export const getListChatRoom = createAsyncThunk(
  "chat/listChatRoom",
  async (userId, { rejectWithValue }) => {
    try {
      const token = localStorage.getItem("token");
      const response = await api.get(`/chat/getListChatRoom?userId=${userId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

export const createChatRoom = createAsyncThunk(
  "chat/createChatRoom",
  async ({ userId, nameRoom, nguoiThamGia }, { rejectWithValue }) => {
    try {
      debugger;
      const token = localStorage.getItem("token");
      const response = await api.post(
        `/chat/createChat`,
        {
          nameRoom,
          nguoiThamGia,
          userId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
