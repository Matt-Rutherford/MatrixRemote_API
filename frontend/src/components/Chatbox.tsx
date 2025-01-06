import React, { useState } from "react";
import "../styles/Chatbox.css";

interface Message {
    id: number;
    content: string;
    timestamp: string;
}

const Chatbox: React.FC = () => {
    const [messages, setMessages] = useState<Message[]>([
        { id: 1, content: "Hello from NY!", timestamp: "10:30 AM" },
        { id: 2, content: "Hello from SF!", timestamp: "11:00 AM" },
    ]);
    const [newMessage, setNewMessage] = useState("");

    const sendMessage = () => {
        if (newMessage.trim()) {
            const timestamp = new Date().toLocaleTimeString();
            setMessages([...messages, { id: messages.length + 1, content: newMessage, timestamp }]);
            setNewMessage("");
        }
    };

    return (
        <div className="chatbox">
            <div className="chatbox-messages">
                {messages.map((message) => (
                    <div key={message.id} className="message">
                        <p><strong>{message.timestamp}:</strong> {message.content}</p>
                    </div>
                ))}
            </div>
            <div className="chatbox-input">
                <input
                    type="text"
                    placeholder="Type your message..."
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                />
                <button onClick={sendMessage}>Send</button>
            </div>
        </div>
    );
};

export default Chatbox;
