import { configureStore } from "@reduxjs/toolkit";
import authReducer from "../Features/authSlice";

import storage from "redux-persist/lib/storage";
import persistReducer from "redux-persist/es/persistReducer";
import userReducer from "../Features/User/userSlice";
import chatReducer from "../Features/Chat/chatSlice";
const authPersistConfig = {
  key: "auth",
  storage,
  whitelist: ["token", "userId"], // chỉ lưu các field cần thiết
};
const persistedAuthReducer = persistReducer(authPersistConfig, authReducer);

const store = configureStore({
  reducer: {
    auth: persistedAuthReducer,
    user: userReducer,
    chat: chatReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        // Bỏ qua các action liên quan đến SignalR connection
        ignoredActions: [
          "persist/PERSIST",
          "persist/REHYDRATE",
          "persist/REGISTER",
          "chat/setConnection",
          "chat/sendMessage/fulfilled",
          "chat/sendMessage/pending",
          "chat/sendMessage/rejected",
        ],
        // Bỏ qua các path trong state chứa connection
        ignoredPaths: ["chat.connection"],
        // Bỏ qua các action types
        ignoredActionsPaths: ["payload.connection", "meta.arg.connection"],
      },
    }),
});
export default store;
