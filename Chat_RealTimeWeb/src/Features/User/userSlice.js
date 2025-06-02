import { createSlice } from "@reduxjs/toolkit";
import { getListUser } from "./userApi";

const userSlice = createSlice({
  name: "user",
  initialState: {
    dataUser: [],
    loading: false,
    error: null,
    userName: "",
   
  },
  reducers: {

  },
  extraReducers: (builder) => {
    builder
      .addCase(getListUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getListUser.fulfilled, (state, action) => {
        state.loading = false;
        state.dataUser = action.payload;
        // state.userName = action.payload.dataUser.userName;
      })
      .addCase(getListUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error?.message;
      });
  },
});

export default userSlice.reducer;
