// src/App.jsx
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HeaderBar from "./Components/header";
import ProtectedRoute from "./Components/protectedProduct";
import Login from "./Pages/login";
import Register from "./Pages/register";
import Dashboard from "./Pages/dashboard";

const App = () => {
  return (
    <Router>
      <div
        style={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}
      >
        <HeaderBar />
        <div
          style={{
            background: "#ececec",
            padding: "20px",
            textAlign: "center",
          }}
        >
          Banner Area
        </div>
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
        </Routes>
        {/* <div
          style={{
            textAlign: "center",
            padding: "20px",
            background: "#f0f2f5",
          }}
        >
          ©2025 My Website
        </div> */}
      </div>
    </Router>
  );
};

export default App;
