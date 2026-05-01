import { useState } from "react";
import {
  sendBroadcastMessage,
  sendMessageToUser,
} from "../services/signalrService";

export default function Mensajes() {
  const [mode, setMode] = useState("all"); // all | user
  const [userId, setUserId] = useState("");
  const [message, setMessage] = useState("");
  const [sending, setSending] = useState(false);
  const [result, setResult] = useState({ type: "", text: "" });

  async function handleSubmit(e) {
    e.preventDefault();
    setResult({ type: "", text: "" });

    const cleanMessage = message.trim();
    const cleanUserId = userId.trim();

    if (!cleanMessage) {
      setResult({ type: "error", text: "Escribe un mensaje." });
      return;
    }

    if (mode === "user" && !cleanUserId) {
      setResult({ type: "error", text: "Escribe el userId destino." });
      return;
    }

    setSending(true);
    try {
      if (mode === "all") {
        await sendBroadcastMessage(cleanMessage);
      } else {
        await sendMessageToUser(cleanUserId, cleanMessage);
      }

      setResult({ type: "ok", text: "Mensaje enviado correctamente." });
      setMessage("");
      if (mode === "all") setUserId("");
    } catch (error) {
      setResult({
        type: "error",
        text: error?.message || "No se pudo enviar el mensaje.",
      });
    } finally {
      setSending(false);
    }
  }

  return (
    <main className="max-w-3xl mx-auto p-3 sm:p-4 md:p-6">
      <div className="bg-white rounded-lg shadow p-5 sm:p-6">
        <h1 className="text-xl font-bold mb-4">Real Time Messaging</h1>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Destino
            </label>
            <div className="flex gap-2">
              <button
                type="button"
                onClick={() => setMode("all")}
                className={`px-3 py-2 rounded border text-sm ${
                  mode === "all"
                    ? "bg-green-600 text-white border-green-600"
                    : "bg-white text-gray-700 border-gray-300"
                }`}
              >
                All
              </button>
              <button
                type="button"
                onClick={() => setMode("user")}
                className={`px-3 py-2 rounded border text-sm ${
                  mode === "user"
                    ? "bg-green-600 text-white border-green-600"
                    : "bg-white text-gray-700 border-gray-300"
                }`}
              >
                To a user
              </button>
            </div>
          </div>

          {mode === "user" && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                User ID
              </label>
              <input
                type="text"
                value={userId}
                onChange={(e) => setUserId(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                placeholder="Ej: 123 o GUID del usuario"
                disabled={sending}
              />
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Mensaje
            </label>
            <textarea
              rows={4}
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              className="w-full border border-gray-300 rounded p-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              placeholder="Escribe el mensaje a enviar"
              disabled={sending}
            />
          </div>

          {result.text && (
            <div
              className={`p-2 rounded text-sm ${
                result.type === "ok"
                  ? "bg-green-100 text-green-700 border border-green-300"
                  : "bg-red-100 text-red-700 border border-red-300"
              }`}
            >
              {result.text}
            </div>
          )}

          <button
            type="submit"
            disabled={sending}
            className="bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white px-4 py-2 rounded font-semibold text-sm"
          >
            {sending ? "Enviando..." : "Send message"}
          </button>
        </form>
      </div>
    </main>
  );
}