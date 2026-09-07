<template>
  <div class="navbar">
    <!-- 左侧：家庭名称 -->
    <div class="logo">
      <span class="icon">🏠</span>
      <span class="title">我的家庭存储系统</span>
    </div>

    <!-- 右侧：用户状态与登录按钮 -->
    <div class="user-area">
      <template v-if="userStore.isLoggedIn">
        <el-tag type="danger" style="margin-right: 10px;">管理员</el-tag>
        <span class="username">👤 {{ userStore.userName }}</span>
        <el-button type="text" class="logout-btn" @click="handleLogout">
          退出登录
        </el-button>
      </template>

      <template v-else>
        <el-tag type="info" style="margin-right: 10px;">游客</el-tag>
        <el-button type="primary" size="small" @click="showLoginModal = true">
          登录
        </el-button>
      </template>
    </div>

    <!-- 登录对话框 -->
    <el-dialog v-model="showLoginModal" title="管理员登录" width="360px">
      <el-form label-position="top">
        <el-form-item label="账号">
          <el-input v-model="loginForm.username" placeholder="请输入账号" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="loginForm.password" type="password" placeholder="请输入密码" show-password />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showLoginModal = false">取消</el-button>
        <el-button type="primary" @click="handleLogin">确认登录</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useUserStore } from '../stores/userStore'
import { ElMessage } from 'element-plus'

const userStore = useUserStore()
const showLoginModal = ref(false)

const loginForm = reactive({
  username: '',
  password: ''
})

const handleLogin = () => {
  if (!loginForm.username || !loginForm.password) {
    ElMessage.warning('请输入账号和密码')
    return
  }

  // 模拟登录：输入任意账号密码即视为管理员身份
  userStore.login(loginForm.username, true)
  showLoginModal.value = false
  ElMessage.success(`欢迎回来，${loginForm.username}！`)

  loginForm.username = ''
  loginForm.password = ''
}

const handleLogout = () => {
  userStore.logout()
  ElMessage.info('已退出登录，切换为游客浏览模式')
}
</script>

<style scoped>
.navbar {
  height: 60px;
  background-color: #ffffff;
  border-bottom: 1px solid #e6e6e6;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.logo {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: bold;
  color: #303133;
}

.icon {
  font-size: 24px;
  margin-right: 8px;
}

.user-area {
  display: flex;
  align-items: center;
}

.username {
  font-size: 14px;
  color: #606266;
  font-weight: 500;
}

.logout-btn {
  color: #f56c6c !important;
  margin-left: 15px;
}
</style>