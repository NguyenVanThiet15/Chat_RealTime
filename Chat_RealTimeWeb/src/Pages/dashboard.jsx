// src/pages/Dashboard.jsx
// import React, { useEffect } from "react";
// import { useDispatch, useSelector } from "react-redux";

import { Button, Card, Col, Layout, List, Row, Tooltip } from "antd";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getListUser } from "../Features/User/userApi";
import { MessageOutlined } from "@ant-design/icons";
import ChatWindow from "./window";
import { getListChatRoom } from "../Features/Chat/chatApi";
import ChatRoom from "./createChatRoom";

const Dashboard = () => {
  const dispatch = useDispatch();

  const user = useSelector((state) => state.auth.userId);

  const isOpenModal = useSelector((state) => state.chat.isOpenModal);
  const { dataUser } = useSelector((state) => state.user);
  const { listChatRoom } = useSelector((state) => state.chat);
  // State để quản lý chat
  const [selectedUser, setSelectedUser] = useState(null);
  const [selectedRoom, setSelectedRoom] = useState(null);
  const [showChat, setShowChat] = useState(false);
  const [chatType, setChatType] = useState(null);

  useEffect(() => {
    if (user) {
      dispatch(getListUser(user));
    }
  }, [user]);

  useEffect(() => {
    if (user) {
      dispatch(getListChatRoom());
    }
  }, [user]);
  const handleSelectUser = (clickedUser) => {
    setSelectedUser(clickedUser);
    setShowChat(true);
    setSelectedRoom(null);
    setChatType("Private");
  };
  const handleSelectRoom = (clickedRoom) => {
    setShowChat(true);
    setSelectedRoom(clickedRoom);
    setSelectedUser(null);
    setChatType("Group");
  };
  const handleCloseChat = () => {
    setShowChat(false);
    setSelectedUser(null);
    selectedRoom(null);
  };

  console.log("selectedRoom", selectedRoom);

  return (
    <>
      <Layout>
        <Row gutter={16} style={{ height: "100%" }}>
          <Col span={6}>
            <Card
              style={{
                height: "100%",
                overflowY: "auto",
                maxHeight: "calc(100vh - 100px)",
                flexDirection: "column", // hoặc chiều cao cố định, ví dụ: 500
              }}
            >
              <div
                style={{
                  flex: 1,
                  overflow: "auto",
                  borderBottom: "1px solid #f0f0f0",
                  padding: 12,
                }}
              >
                <List
                  itemLayout="horizontal"
                  dataSource={listChatRoom}
                  renderItem={(itemRoom) => {
                    return (
                      <List.Item
                        onClick={() => handleSelectRoom(itemRoom.id)}
                        style={{ cursor: "pointer" }}
                        actions={[
                          <Button
                            type="text"
                            icon={<MessageOutlined />}
                            onClick={(e) => {
                              e.stopPropagation();
                              handleSelectRoom(itemRoom);
                            }}
                          ></Button>,
                        ]}
                      >
                        <List.Item.Meta
                          title={
                            <Tooltip title={itemRoom.name}>
                              <div
                                style={{
                                  whiteSpace: "nowrap",
                                  overflow: "hidden",
                                  textOverflow: "ellipsis",
                                  maxWidth: "200px",
                                }}
                              >
                                {itemRoom.name}
                              </div>
                            </Tooltip>
                          }
                        />
                      </List.Item>
                    );
                  }}
                ></List>
              </div>
              <div
                style={{
                  flex: 1,
                  overflow: "auto",
                  padding: 12,
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
              </div>
            </Card>
          </Col>
          <Col span={18}>
            {showChat && (selectedUser || selectedRoom) ? (
              <ChatWindow
                nguoiNhan={selectedUser}
                nhomChatRoom={selectedRoom}
                nguoiGuiID={user}
                chatType={chatType}
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
                  <p>
                    Chọn một người dùng từ danh sách bên trái để bắt đầu chat
                  </p>
                </div>
              </Card>
            )}
          </Col>
        </Row>
      </Layout>

      {isOpenModal && <ChatRoom />}
    </>
  );
};

export default Dashboard;
