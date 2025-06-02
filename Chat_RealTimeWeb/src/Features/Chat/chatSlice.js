import { createSlice } from "@reduxjs/toolkit";
import {
  createChatRoom,
  createOrGetChat,
  getListChatRoom,
  getMessage,
  sendMessageImage,
} from "./chatApi";

const chaSlice = createSlice({
  name: "chat",
  initialState: {
    currentChat: null,
    messages: [],
    loading: {
      chat: false,
      messages: false,
      sending: false,
    },
    error: null,
    connection: null,
    typingUser: [],
    pagination: {
      page: 1,
      hasMore: true,
    },
    // nameChat: "",
    // chatRoomId: "",
    listChatRoom: [],
    isOpenModal: false,
  },
  reducers: {
    //thiết lập signlar
    setConnection: (state, action) => {
      state.connection = action.payload;
    },
    clearConnection: (state) => {
      if (state.connection) {
        state.connection.stop();
      }
      state.connection = null;
    },
    receiveMessage: (state, action) => {
      const message = action.payload;
      const exists = state.messages.find((m) => m.id === message.id);
      if (!exists) {
        state.messages.push(message);
      }
    },
    setUserTyping: (state, action) => {
      const { userId, isTyping } = action.payload;
      if (isTyping) {
        if (!state.typingUser.includes(userId)) {
          state.typingUser.push(userId);
        }
      } else {
        state.typingUser = state.typingUser.filter((id) => id !== userId);
      }
    },
    // Clear chat hiện tại
    clearCurrentChat: (state) => {
      state.currentChat = null;
      state.messages = [];
      state.typingUsers = [];
      state.pagination = { page: 1, hasMore: true };
    },
    openModal: (state) => {
      state.isOpenModal = true;
    },
    closeModal: (state) => {
      state.isOpenModal = false;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(createOrGetChat.pending, (state) => {
        state.loading.chat = true;
        state.error = null;
      })
      .addCase(createOrGetChat.fulfilled, (state, action) => {
        state.loading.chat = false;
        state.currentChat = action.payload;
        state.messages = [];
        // state.pagination = { page: 1, hasMore: true };
      })
      .addCase(createOrGetChat.rejected, (state, action) => {
        state.loading.chat = false;
        state.error = action.payload;
      });
    //sendMessage
    // builder
    //   .addCase(sendMessage.pending, (state) => {
    //     state.loading.sending = true;
    //     state.error = null;
    //   })
    //   .addCase(sendMessage.fulfilled, (state, action) => {
    //     state.loading.sending = false;
    //     // tin nhắn sẽ đc gửi qua signlar qua receiveMessage
    //   })
    //   .addCase(sendMessage.rejected, (state, action) => {
    //     state.loading.sending = false;
    //     state.error = action.payload;
    //   });
    builder
      .addCase(getMessage.pending, (state) => {
        state.loading.messages = false;
        state.error = null;
      })
      .addCase(getMessage.fulfilled, (state, action) => {
        state.loading.messages = false;
        state.messages = action.payload;
      })
      .addCase(getMessage.rejected, (state, action) => {
        state.loading.messages = false;
        state.error = action.payload;
      });
    // sendMessageImage
    builder
      .addCase(sendMessageImage.pending, (state) => {
        state.loading.sending = true;
        state.error = null;
      })
      .addCase(sendMessageImage.fulfilled, (state, action) => {
        state.loading.sending = false;
        // tin nhắn sẽ đc gửi qua signlar qua receiveMessage
      })
      .addCase(sendMessageImage.rejected, (state, action) => {
        state.loading.sending = false;
        state.error = action.payload;
      });
    //list chat room
    builder
      .addCase(getListChatRoom.pending, (state) => {
        state.loading.sending = true;
        state.error = null;
      })
      .addCase(getListChatRoom.fulfilled, (state, action) => {
        state.loading.sending = false;
        // state.nameChat = action.payload.name;
        // state.chatRoomId = action.payload.id;
        state.listChatRoom = action.payload;
      })
      .addCase(getListChatRoom.rejected, (state, action) => {
        state.loading.sending = false;
        state.error = action.payload;
      });
    // create Chat room
    builder
      .addCase(createChatRoom.pending, (state) => {
        state.loading.sending = true;
        state.error = null;
      })
      .addCase(createChatRoom.fulfilled, (state, action) => {
        state.loading.sending = false;
        state.listChatRoom = action.payload;
        state.isOpenModal = false;
      })
      .addCase(createChatRoom.rejected, (state, action) => {
        state.loading.sending = false;
        state.error = action.payload;
        state.isOpenModal = false;
      });
  },
});

export const {
  setConnection,
  clearConnection,
  receiveMessage,
  setUserTyping,
  clearCurrentChat,
  openModal,
  closeModal,
} = chaSlice.actions;
export default chaSlice.reducer;
