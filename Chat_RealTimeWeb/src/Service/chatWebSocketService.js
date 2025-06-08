class ChatWebSocketService {
  constructor() {
    this.ws = null;
    this.reconnectAttempts = 0;
    this.maxReconnectAttempts = 5;
    this.reconnectInterval = 3000;
    this.listeners = new Map();
    this.isConnected = false;
    this.currentChatId = null;
    this.userId = null;
  }
  connect(userId) {
    this.userId = userId;
    const token = localStorage.getItem("token");

    this.ws = new WebSocket(
      `wss://localhost:7152/ws?token=${token}&userId=${userId}`
    );
    this.ws.onopen = () => {
      console.log("WebSocket connected");
      this.isConnected = true;
      this.reconnectAttempts = 0;

      if (this.currentChatId) {
        this.joinChat(this.currentChatId);
      }
    };
    this.ws.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        this.handleMessage(data);
      } catch (error) {
        console.error("Error parsing WebSocket message:", error);
      }
    };
    this.ws.onclose = () => {
      console.log("websocket disconneted");
      this.isConnected = false;
      this.attemptReconnect();
    };
    this.ws.onerror = (error) => {
      console.error("WebSocket error:", error);
    };
  }
  attemptReconnect() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      setTimeout(() => {
        console.log(`Attemping to reconnect... 
                (${this.reconnectAttempts + 1}/${this.maxReconnectAttempts})`);
        this.reconnectAttempts++;
        this.connect(this.userId);
      }, this.reconnectInterval);
    }
  }
  handleMessage(data) {
    const { type, chatId, payload } = data;

    switch (type) {
      case "ReceiveMessage":
        this.emit("messageReceived", payload);
        break;
      case "UserJoined":
        this.emit("userJoined", payload);
        break;
      case "UserLeft":
        this.emit("userLeft", payload);
        break;
      case "UserTyping":
        this.emit("userTyping", payload);
        break;
      case "LoadMessages":
        this.emit("loadMessages", payload);
        break;
      case "Error":
        this.emit("error", payload);
        break;
      default:
        console.log("Unknow message type", type);
    }
  }
  joinChat(chatId) {
    debugger;
    this.currentChatId = chatId;
    if (this.isConnected) {
      this.send({
        type: "JoinChat",
        chatId: chatId,
        userId: this.userId,
      });
    }
  }
  sendMessage(chatId, content) {
    this.send({
      type: "SendMessage",
      chatId: chatId,
      senderId: this.userId,
      content: content,
    });
  }
  setTyping(chatId, isTyping) {
    this.send({
      type: "Typing",
      chatId: chatId,
      userId: this.userId,
      isTyping: isTyping,
    });
  }
  leaveChat(chatId) {
    this.send({
      type: "Leave",
      chatId: chatId,
      userId: this.userId,
    });
  }
  send(data) {
    if (this.ws && this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify(data));
    } else {
      console.warn("websocket is not connected !");
    }
  }

  on(event, callback) {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, []);
    }
    this.listeners.get(event).push(callback);
  }
  off(event, callback) {
    if (this.listeners.has(event)) {
      const callbacks = this.listeners.get(event);
      const index = callbacks.indexOf(callback);
      if (index > -1) {
        callbacks.splice(index, 1);
      }
    }
  }
  emit(event, data) {
    if (this.listeners.has(event)) {
      this.listeners.get(event).forEach((callback) => callback(data));
    }
  }
  disconnect() {
    if (this.ws) {
      this.ws.close();
      this.ws = null;
    }
    this.isConnected = false;
    this.currentChatId = null;
  }
}
export default new ChatWebSocketService();
