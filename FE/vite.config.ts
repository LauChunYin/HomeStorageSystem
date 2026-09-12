import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      // 匹配所有以 /api 开头的请求
      '/api': {
        target: 'http://localhost:5230', //.NET 8 后端 API 地址
        changeOrigin: true,
        secure: false // 如果后端用的是 https 自签名证书，设为 false
      }
    }
  }
})
