import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
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
      const response = await api.get(`/chat/${chatId}/getMessage`);
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
      const response = await api.post(`/chat/${chatId}/SendMessageImg`, {
        imageFile,
        senderId,
      });
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

export const joinChatRedis = createAsyncThunk(
  "chatredis/joinChatRedis",
  async ({ chatId, userId }, { rejectWithValue }) => {
    try {
      
      const response = await api.post(`/ChatRedis/join/${chatId}`, { userId });
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

export const SendMessageChatRedis = createAsyncThunk(
  "chatredis/SendMessageChatRedis",
  async ({ chatId, content, senderId }, { rejectWithValue }) => {
    try {
      const response = await api.post(`chatRedis/sendMessageRedis`, {
        chatId,
        content,
        senderId,
      });
      return response.data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);
