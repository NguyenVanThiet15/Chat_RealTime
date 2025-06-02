// src/features/auth/authSlice.js
import { createSlice } from "@reduxjs/toolkit";
import { loginByGoogle, loginUser, registerUser } from "./authApi";

const authSlice = createSlice({
  name: "auth",
  initialState: {
    userId: null,
    token: localStorage.getItem("token") || null,
    loading: false,
    error: null,
    isAuthenticated: !!localStorage.getItem("token"),
    user: JSON.parse(localStorage.getItem("user")) || null,
    tokenGoogle: null,
  },
  reducers: {
    logout: (state) => {
      state.userId = null;
      state.token = null;
      localStorage.removeItem("token");
      localStorage.removeItem("userId");
      state.isAuthenticated = false;
    },
    restoreAuth: (state) => {
      const token = localStorage.getItem("token");
      const user = localStorage.getItem("user");
      if (token && user) {
        state.token = token;
        state.user = JSON.parse(user);
        state.isAuthenticated = true;
      }
    },
  },
  extraReducers: (builder) => {
    builder
      // Đăng ký
      .addCase(registerUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(registerUser.fulfilled, (state, action) => {
        state.loading = false;
        state.userId = action.payload.userId;
        state.token = action.payload.token;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "Đăng ký thất bại";
      })
      // Đăng nhập
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(loginUser.fulfilled, (state, action) => {
        debugger;
        state.loading = false;
        state.userId = action.payload.userId;
        state.token = action.payload.token;
        state.user = {
          id: action.payload.userId,
          useName: action.payload.useName,
          email: action.payload.email,
        };
        state.isAuthenticated = true;
        // localStorage.setItem("token", action.payload.token);
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "Đăng nhập thất bại";
      });
    builder
      .addCase(loginByGoogle.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(loginByGoogle.fulfilled, (state, action) => {
        state.userId = action.payload.userId;
        state.tokenGoogle = action.payload.tokenGoogle;
      })
      .addCase(loginByGoogle.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "Đăng nhập thất bại";
      });
  },
});

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;
