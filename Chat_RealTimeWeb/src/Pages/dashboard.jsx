// src/pages/Dashboard.jsx
// import React, { useEffect } from "react";
// import { useDispatch, useSelector } from "react-redux";

import { Avatar, Button, Card, Col, Layout, List, Row, Tooltip } from "antd";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getListUser } from "../Features/User/userApi";
import { MessageOutlined } from "@ant-design/icons";
import ChatWindow from "./window";

const Dashboard = () => {
  const dispatch = useDispatch();

  const user = useSelector((state) => state.auth.userId);
  const { dataUser } = useSelector((state) => state.user);
  // State để quản lý chat
  const [selectedUser, setSelectedUser] = useState(null);
  const [showChat, setShowChat] = useState(false);

  useEffect(() => {
    if (user) {
      dispatch(getListUser(user));
    }
  }, [user]);
  const handleSelectUser = (clickedUser) => {
    setSelectedUser(clickedUser);
    setShowChat(true);
  };
  const handleCloseChat = () => {
    setShowChat(false);
    setSelectedUser(null);
  };

  return (
    <Layout>
      <Row gutter={16} style={{ height: "100%" }}>
        <Col span={6}>
          <Card
            style={{
              height: "100%",
              overflowY: "auto",
              maxHeight: "calc(100vh - 100px)", // hoặc chiều cao cố định, ví dụ: 500
            }}
          >
            <List
              itemLayout="horizontal"
              dataSource={dataUser}
              renderItem={(item) => (
                <List.Item
                  onClick={() => handleSelectUser(item.userId)}
                  style={{ cursor: "pointer" }}
                  className="user-list-item"
                  actions={[
                    <Button
                      type="text"
                      icon={<MessageOutlined />}
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleSelectUser(item);
                      }}
                    />,
                  ]}
                >
                  <List.Item.Meta
                    // avatar={<Avatar src={item.avatar} />}
                    // title={item.userName}
                    title={
                      <Tooltip title={item.userName}>
                        <div
                          style={{
                            whiteSpace: "nowrap",
                            overflow: "hidden",
                            textOverflow: "ellipsis",
                            maxWidth: "200px",
                          }}
                        >
                          {item.userName}
                        </div>
                      </Tooltip>
                    }
                  />
                </List.Item>
              )}
            ></List>
          </Card>
        </Col>
        <Col span={18}>
          {showChat && selectedUser ? (
            <ChatWindow
              targetUser={selectedUser}
              currentUser={user}
              onClose={handleCloseChat}
            />
          ) : (
            <Card style={{ height: "100%" }}>
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  height: "100%",
                  flexDirection: "column",
                  color: "#8c8c8c",
                }}
              >
                <MessageOutlined style={{ fontSize: 64, marginBottom: 16 }} />
                <h3>Chọn một người để bắt đầu trò chuyện</h3>
                <p>Chọn một người dùng từ danh sách bên trái để bắt đầu chat</p>
              </div>
            </Card>
          )}
        </Col>
      </Row>
    </Layout>
  );
};

export default Dashboard;
