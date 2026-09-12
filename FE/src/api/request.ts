import axios from 'axios'
import { ElMessage } from 'element-plus'

// 创建 axios 实例
const request = axios.create({
  baseURL: '/api', // 所有请求自动带上 /api 前缀，被 vite 代理拦截
  timeout: 10000   // 10秒超时
})

// 响应拦截器：统一处理报错
request.interceptors.response.use(
  (response) => {
    return response.data
  },
  (error) => {
    console.error('API Error:', error)
    const message = error.response?.data?.message || '网络请求错误，请检查后端是否启动'
    ElMessage.error(message)
    return Promise.reject(error)
  }
)

export default request