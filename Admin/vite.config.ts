import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: './docs'
  },
  server: {
    proxy: {
      "/admin": "http://localhost:5151"
    }
  },
  resolve: {
    alias: {
      "shared": path.resolve(__dirname, "./src/shared")
    }
  }
})
