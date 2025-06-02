// src/main.jsx
import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { Provider } from "react-redux";
import store from "./Store/index";
import "antd/dist/reset.css";
import { GoogleOAuthProvider } from "@react-oauth/google";

ReactDOM.createRoot(document.getElementById("root")).render(
  <Provider store={store}>
    <GoogleOAuthProvider clientId="238327738777-kj7jorahkjv6afjjntikitthfu5jk1q0.apps.googleusercontent.com">
      <App />
    </GoogleOAuthProvider>
  </Provider>
);
