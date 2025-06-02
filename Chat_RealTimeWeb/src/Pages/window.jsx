import {
  Avatar,
  Button,
  Card,
  Input,
  List,
  Space,
  Spin,
  Typography,
  Upload,
} from "antd";
import { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  createOrGetChat,
  getMessage,
  sendMessageImage,
} from "../Features/Chat/chatApi";
import { SendOutlined, UploadOutlined } from "@ant-design/icons";
import * as signalR from "@microsoft/signalr";
import {
  clearConnection,
  receiveMessage,
  setConnection,
  setUserTyping,
} from "../Features/Chat/chatSlice";
import ChatRoom from "./createChatRoom";
const ChatWindow = ({ nguoiNhan, nguoiGuiID, onClose, nhomChatRoom }) => {
  const { currentChat, messages, loading, connection } = useSelector(
    (state) => state.chat
  );
  const isOpenModal = useSelector((state) => state.chat.isOpenModal);
  const dispatch = useDispatch();
  const [messageText, setMessageText] = useState("");
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef(null);
  const typingTimeoutRef = useRef(null);
  const { Text } = Typography;
  // console.log("nhomChatRoom", nhomChatRoom);
  useEffect(() => {
    debugger;
    if (nguoiNhan && nguoiGuiID && nhomChatRoom) {
      dispatch(
        createOrGetChat({
          nguoiGuiId: nguoiGuiID,
          nguoiNhanId: nguoiNhan.id,
          nameRoom: nhomChatRoom.nameRoom,
          roomId: nhomChatRoom.id,
          participant: nhomChatRoom.participants,
        })
      );
    }
  }, [nguoiNhan, nguoiGuiID, nhomChatRoom]);

  const handleTyping = (e) => {
    setMessageText(e.target.value);

    if (!isTyping && connection && currentChat) {
      setIsTyping(true);
      connection.invoke("Typing", currentChat.id, nguoiGuiID, true);
    }

    // Clear timeout cũ
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    // Set timeout mới
    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      if (connection && currentChat) {
        connection.invoke("Typing", currentChat.id, nguoiGuiID, false);
      }
    }, 2000);
  };

  useEffect(() => {
    const setupSignalR = async () => {
      // debugger;
      const token = localStorage.getItem("token");
      if (!token) {
        console.log("không tìm thấy token ");
        return;
      }
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7152/chathub", {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect([0, 2000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();
      // newConnection.serverTimeoutInMilliseconds = 120000;
      // newConnection.keepAliveIntervalInMilliseconds = 30000;
      try {
        newConnection.onclose((error) => {
          console.log("Signlar đã đóng ", error);
        });
        newConnection.onreconnecting((error) => {
          console.log("đang thử kết nối lại ", error);
        });
        newConnection.onreconnected((error) => {
          console.log("đã kết nối thành công", error);
        });
        await newConnection.start().catch((err) => {
          console.error("lỗi signlar", err);
        });
        dispatch(setConnection(newConnection));
        newConnection.on("ReceiveMessage", (message) => {
          dispatch(receiveMessage(message));
        });
        newConnection.on("Usertyping", (userId, isTyping) => {
          dispatch(setUserTyping({ userId, isTyping }));
        });
      } catch (error) {
        console.error("SignalR connection failed:", error);
      }
    };
    setupSignalR();
    return () => {
      dispatch(clearConnection());
    };
  }, [dispatch]);

  // console.log("curuntchat", currentChat);
  // console.log("message", messages);
  console.log("nguoiNhan", nguoiNhan);

  const handleSendMessage = async () => {
    if (!messageText.trim() || !currentChat) return;

    if (connection) {
      await connection.invoke(
        "SendMessage",
        currentChat.id,
        nguoiGuiID,
        messageText
      );
    }
    setMessageText("");
  };
  // const handleSendMessageImg = async (info) => {
  //   debugger;
  //   const file = info.file.originFileObj;
  //   if (!file) return;
  //   dispatch(
  //     sendMessageImage({
  //       chatId: currentChat.id,
  //       imageFile: file,
  //       senderId: nguoiGuiID,
  //     })
  //   );
  // };
  const formatTime = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleTimeString("vi-VN", {
      hour: "2-digit",
      minute: "2-digit",
    });
  };
  useEffect(() => {
    if (currentChat && connection) {
      connection.invoke("JoinChat", currentChat.id, nguoiGuiID);
      dispatch(getMessage({ chatId: currentChat.id }));
    }
  }, [currentChat, connection, nguoiGuiID]);

  return (
    <>
      <Card
        style={{ height: "100%", display: "flex", flexDirection: "column" }}
        bodyStyle={{
          padding: 0,
          height: "100%",
          display: "flex",
          flexDirection: "column",
        }}
        title={
          <Space>
            {/* <Avatar size="small">{targetUser.userName}</Avatar> */}
            <div>
              <Text strong>{nguoiNhan.userName} </Text>
              <br />
              <Text type="secondary" style={{ fontSize: 12 }}>
                {/* {typingUsers.includes(targetUser.userId) ? 'Đang gõ...' : 'Trực tuyến'} */}
              </Text>
            </div>
          </Space>
        }
        extra={
          <Button
            type="text"
            //   icon={<CloseOutlined />}
            onClick={onClose}
            size="small"
          />
        }
      >
        <div
          style={{
            flex: 1,
            overflow: "auto",
            padding: "16px",
            maxHeight: "calc(100% - 120px)",
          }}
        >
          {loading.messages && (
            <div style={{ textAlign: "center", padding: 20 }}>
              <Spin />
            </div>
          )}
        </div>
        <List
          dataSource={messages}
          renderItem={(message) => (
            <List.Item
              style={{
                border: "none",
                padding: "4px 0",
                justifyContent:
                  message.senderId === nguoiGuiID ? "flex-end" : "flex-start",
              }}
            >
              <div
                style={{
                  border: "1px solid #ccc",
                  borderRadius: "10px",
                  padding: "10px",
                  lineHeight: "1.8",
                }}
              >
                {message.content}
              </div>
              <div
                style={{
                  fontSize: 11,
                  opacity: 0.7,
                  marginTop: 4,
                  textAlign: "right",
                }}
              >
                {formatTime(message.createdAt)}
                {message.senderId === nguoiGuiID && (
                  <span style={{ marginLeft: 4 }}>
                    {message.isRead ? "✓✓" : "✓"}
                  </span>
                )}
              </div>
            </List.Item>
          )}
        ></List>
        <div ref={messagesEndRef} />
        <div style={{ borderTop: "1px solid #f0f0f0" }}>
          {/* <Space.Compact style={{ width: "100%" }}>
         
            <Upload
              accept="image/*"
              showUploadList={false}
              beforeUpload={() => false}
              onChange={handleSendMessageImg}
            >
              <Button icon={<UploadOutlined />}>Gửi ảnh</Button>
            </Upload>
          </Space.Compact> */}
          <Space.Compact style={{ width: "100%" }}>
            <Input.TextArea
              value={messageText}
              onChange={(e) => handleTyping(e)}
              // onKeyPress={handleKeyPress}
              placeholder="Nhập tin nhắn..."
              autoSize={{ minRows: 1, maxRows: 3 }}
              style={{ resize: "none" }}
              disabled={loading.sending}
            ></Input.TextArea>
            <Button
              type="primary"
              icon={<SendOutlined />}
              onClick={handleSendMessage}
              disabled={!messageText.trim() || loading.sending}
              loading={loading.sending}
            ></Button>
          </Space.Compact>
        </div>
      </Card>
      {isOpenModal && <ChatRoom />}
    </>
  );
};
export default ChatWindow;
