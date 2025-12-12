import { useState } from "react";
import { FiSend } from "react-icons/fi";

export const SupportPage = () => {
  const [message, setMessage] = useState("");

  const handleSend = () => {
    console.log("Сообщение отправлено:", message);
    setMessage("");
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <div
        className="w-full pt-10 pb-10 pl-5 flex flex-col justify-end items-start"
        style={{ backgroundColor: "#3B82F6", minHeight: "220px" }}
      >
        <h1 className="text-5xl font-bold text-white">Поддержка</h1>
        <p className="text-white opacity-90 mt-2 text-2xl">
          Напиши преподавателю
        </p>
      </div>

      <div className="max-w-3xl mx-5 mt-8 bg-white p-6 rounded-xl shadow-md">
        <h2 className="text-2xl font-semibold mb-4">Твоё сообщение</h2>
        <textarea
          className="w-full h-40 p-3 border border-gray-300 rounded-lg resize-none focus:outline-none focus:ring-2 focus:ring-blue-400 focus:border-blue-400"
          placeholder="Напиши здесь своё сообщение..."
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />

        <button
          onClick={handleSend}
          className="mt-4 flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-medium transition-colors"
        >
          <FiSend className="w-5 h-5" />
          Отправить
        </button>
      </div>

      <div className="max-w-3xl mx-5 mt-6 p-4 text-gray-600 text-base">
        <p>
          Пока это визуальная страница. В будущем здесь можно будет прикреплять
          файлы, смотреть историю сообщений и получать ответы от преподавателя.
        </p>
      </div>
    </div>
  );
};
