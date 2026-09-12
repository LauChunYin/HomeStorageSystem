<template>
  <div class="sidebar">
    <div class="sidebar-header">
      <span>物品分类</span>
      <!-- 仅管理员显示新增按钮 -->
      <el-button 
        v-if="userStore.isAdmin" 
        type="primary" 
        link 
        :icon="Plus" 
        @click="showAddDialog = true"
      >
        新增
      </el-button>
    </div>

    <!-- 分类菜单列表 -->
    <el-menu
      :default-active="activeCategory"
      class="category-menu"
      @select="handleSelect"
    >
      <el-menu-item index="all">
        <el-icon><Grid /></el-icon>
        <span>全部物品</span>
      </el-menu-item>

      <el-menu-item 
        v-for="item in categories" 
        :key="item.id" 
        :index="item.id.toString()"
      >
        <el-icon><Folder /></el-icon>
        <span>{{ item.name }}</span>
      </el-menu-item>
    </el-menu>

    <!-- 新增分类弹窗 -->
    <el-dialog v-model="showAddDialog" title="新增分类" width="300px">
      <el-input v-model="newCategoryName" placeholder="请输入分类名称" />
      <template #footer>
        <el-button @click="showAddDialog = false">取消</el-button>
        <el-button type="primary" @click="handleAddCategory">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Plus, Grid, Folder } from '@element-plus/icons-vue'
import { useUserStore } from '../stores/userStore'
import { ElMessage } from 'element-plus'
import type { Category } from '../types/item'
import { getCategoriesApi, createCategoryApi } from '../api/storage' // 引入真实 API

const userStore = useUserStore()
const activeCategory = ref('all')
const showAddDialog = ref(false)
const newCategoryName = ref('')
const categories = ref<Category[]>([]) // 初始化为空，等接口返回

// 定义向父组件传递的事件（当用户点击分类时）
const emit = defineEmits(['select-category'])

// 从后端获取分类列表
const loadCategories = async () => {
  try {
    const res = await getCategoriesApi()
    categories.value = res
  } catch (error) {
    console.error('获取分类失败', error)
  }
}

// 页面挂载时调用接口
onMounted(() => {
  loadCategories()
})

const handleSelect = (index: string) => {
  activeCategory.value = index
  emit('select-category', index)
}

const handleAddCategory = async () => {
  if (!newCategoryName.value.trim()) {
    ElMessage.warning('分类名称不能为空')
    return
  }

  try {
    await createCategoryApi({
      name: newCategoryName.value,
      isPublic: true
    })
    ElMessage.success('添加分类成功')
    newCategoryName.value = ''
    showAddDialog.value = false
    // 重新加载分类列表
    loadCategories()
  } catch (error) {
    console.error('添加分类失败', error)
  }
}
</script>

<style scoped>
.sidebar {
  width: 220px;
  background-color: #fff;
  border-right: 1px solid #e6e6e6;
  height: calc(100vh - 60px);
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
  border-bottom: 1px solid #f0f0f0;
}

.category-menu {
  border-right: none;
  flex: 1;
}
</style>