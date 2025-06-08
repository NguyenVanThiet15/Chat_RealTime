import { useEffect, useRef } from "react";
import { useDispatch } from "react-redux";
import ChatWebSocketService from "../Service/chatWebSocketService";
import { receiveMessage, setUserTyping } from "../Features/Chat/chatSlice";

const UseWebSocket = (userId) => {
  const dispatch = useDispatch();
  const chatServiceRef = useRef(null);

  useEffect(() => {
    if (!userId) return;

    ChatWebSocketService.connect(userId);
    chatServiceRef.current = true;

    const handleMessageReceived = (message) => {
      dispatch(receiveMessage(message));
    };
    const handleUsertyping = (data) => {
      dispatch(
        setUserTyping({
          userId: data.userId,
          isTyping: data.isTyping,
        })
      );
    };
    const handleUserjoined = (data) => {
      console.log(`${data.userName} da tham gia nhom chat `);
    };
    const handleUserLeft = (data) => {
      console.log(`${data.userName} da roi nhom chat`);
    };
    const handleError = (error) => {
      console.log("Chat error", error);
    };

    ChatWebSocketService.on("messageReceived", handleMessageReceived);
    ChatWebSocketService.on("userTyping", handleUsertyping);
    ChatWebSocketService.on("userjoined", handleUserjoined);
    ChatWebSocketService.on("userLeft", handleUserLeft);
    ChatWebSocketService.on("error", handleError);

    return () => {
      ChatWebSocketService.off("messageReceived", handleMessageReceived);
      ChatWebSocketService.off("userTyping", handleUsertyping);
      ChatWebSocketService.off("userjoined", handleUserjoined);
      ChatWebSocketService.off("userLeft", handleUserLeft);
      ChatWebSocketService.off("error", handleError);
      ChatWebSocketService.disconnect();
    };
  }, [userId]);
  return ChatWebSocketService;
};
export default UseWebSocket;
