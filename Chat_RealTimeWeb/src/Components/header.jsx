// src/components/Header.jsx
import React from "react";
import { Menu, message } from "antd";
import { Link } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "../Features/authSlice";
import { openModal } from "../Features/Chat/chatSlice";

const HeaderBar = () => {
  const user = useSelector((state) => state.auth.userId);
  const userLogin = useSelector((state) => state.auth.user);
  const dispatch = useDispatch();

  const handleLogout = () => {
    dispatch(logout());
    message.success("Đã đăng xuất");
  };
  const onChangeModal = () => {
    debugger;
    dispatch(openModal());
  };
  console.log("userLogin", userLogin.username);
  return (
    <div
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        background: "#3366CC",
      }}
    >
      <div style={{ color: "white", padding: "0 20px", fontWeight: "bold" }}>
        Xin Chào {userLogin.userName}
      </div>
      <Menu background="#3366CC" mode="horizontal">
        {!user ? (
          <>
            <Menu.Item key="login">
              <Link to="/login">Đăng nhập</Link>
            </Menu.Item>
            <Menu.Item key="register">
              <Link to="/register">Đăng ký</Link>
            </Menu.Item>
          </>
        ) : (
          <>
            <Menu.Item key="logout" onClick={handleLogout}>
              Đăng xuất ({user.username})
            </Menu.Item>
            <Menu.Item key="/createChatRoom" onClick={onChangeModal}>
              Tạo nhóm
            </Menu.Item>
          </>
        )}
      </Menu>
    </div>
  );
};

export default HeaderBar;
