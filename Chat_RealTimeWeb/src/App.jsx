// src/App.jsx
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HeaderBar from "./Components/header";
import ProtectedRoute from "./Components/protectedProduct";
import Login from "./Pages/login";
import Register from "./Pages/register";
import Dashboard from "./Pages/dashboard";
import ChatRoom from "./Pages/createChatRoom";

const App = () => {
  return (
    <Router>
      <div
        style={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}
      >
        <HeaderBar />
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          {/* <Route path="/createChatRoom" element={<ChatRoom />} /> */}
        </Routes>
       
      </div>
    </Router>
  );
};

export default App;
