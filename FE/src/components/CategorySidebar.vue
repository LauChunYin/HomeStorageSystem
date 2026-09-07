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
import { ref } from 'vue'
import { Plus, Grid, Folder } from '@element-plus/icons-vue'
import { useUserStore } from '../stores/userStore'
import { ElMessage } from 'element-plus'
import type { Category } from '../types/item'

const userStore = useUserStore()
const activeCategory = ref('all')
const showAddDialog = ref(false)
const newCategoryName = ref('')

// 定义向父组件传递的事件（当用户点击分类时）
const emit = defineEmits(['select-category'])

// 模拟分类数据
const categories = ref<Category[]>([
  { id: 1, name: '食品饮料', isPublic: true },
  { id: 2, name: '日常药品', isPublic: true },
  { id: 3, name: '数码工具', isPublic: true },
  { id: 4, name: '重要证件', isPublic: false } // 隐私分类
])

const handleSelect = (index: string) => {
  activeCategory.value = index
  emit('select-category', index)
}

const handleAddCategory = () => {
  if (!newCategoryName.value.trim()) {
    ElMessage.warning('分类名称不能为空')
    return
  }
  categories.value.push({
    id: Date.now(),
    name: newCategoryName.value,
    isPublic: true
  })
  newCategoryName.value = ''
  showAddDialog.value = false
  ElMessage.success('添加分类成功')
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