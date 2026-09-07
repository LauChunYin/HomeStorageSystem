import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore('user', () => {
  const isLoggedIn = ref(false)              // 是否已登录
  const userName = ref('游客')               // 用户名称
  const isAdmin = ref(false)                 // 是否拥有管理员权限

  // 登录逻辑
  const login = (name: string, admin: boolean) => {
    isLoggedIn.value = true
    userName.value = name
    isAdmin.value = admin
  }

  // 退出逻辑
  const logout = () => {
    isLoggedIn.value = false
    userName.value = '游客'
    isAdmin.value = false
  }

  return {
    isLoggedIn,
    userName,
    isAdmin,
    login,
    logout
  }
})