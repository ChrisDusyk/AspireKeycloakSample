import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    https: {
      cert: "./certificates/aspnet_https.pem",
      key: "./certificates/aspnet_https.key",
    },
    port: 3000,
  },
});
