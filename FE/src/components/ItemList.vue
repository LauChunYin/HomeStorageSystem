<template>
  <div class="item-list-container">
    <!-- 顶部操作栏：搜索框 + 新增物品按钮 -->
    <div class="toolbar">
      <el-input
        v-model="searchQuery"
        placeholder="搜索物品名称..."
        :prefix-icon="Search"
        clearable
        class="search-input"
      />
      
      <!-- 仅管理员可添加物品 -->
      <el-button 
        v-if="userStore.isAdmin" 
        type="primary" 
        :icon="Plus"
        @click="showAddModal = true"
      >
        添加物品
      </el-button>
    </div>

    <!-- 物品卡片网格区域 -->
    <div v-if="filteredItems.length > 0" class="grid-container">
      <el-card 
        v-for="item in filteredItems" 
        :key="item.id" 
        class="item-card" 
        :body-style="{ padding: '0px' }"
        shadow="hover"
      >
        <!-- 物品封面图 -->
        <div class="image-wrapper">
          <img :src="item.imageUrl || defaultImage" class="item-image" />
          <el-tag 
            v-if="!item.isPublic" 
            type="warning" 
            size="small" 
            class="private-tag"
          >
            仅管理员
          </el-tag>
        </div>

        <!-- 物品信息 -->
        <div class="item-info">
          <div class="item-header">
            <h4 class="item-name">{{ item.name }}</h4>
            <el-tag size="small" type="info">{{ item.category }}</el-tag>
          </div>
          
          <p class="item-location">
            📍 存储位置：{{ item.location || '未指定' }}
          </p>

          <div class="item-footer">
            <span class="quantity">数量：<strong>{{ item.quantity }}</strong></span>
            
            <!-- 管理员操作按钮 -->
            <div v-if="userStore.isAdmin" class="actions">
              <el-button type="danger" link size="small" @click="handleDelete(item.id)">
                删除
              </el-button>
            </div>
          </div>
        </div>
      </el-card>
    </div>

    <!-- 无数据时的空状态 -->
    <el-empty v-else description="暂无相关物品" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Search, Plus } from '@element-plus/icons-vue'
import { useUserStore } from '../stores/userStore'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { Item } from '../types/item'

// 接收父组件传递的筛选条件
const props = defineProps<{
  categoryId: string
  location: string
}>()

const userStore = useUserStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const defaultImage = 'https://cube.elemecdn.com/e/fd/0ce71b31860b5f5509000926919c7png.png'

// 模拟物品 Mock 数据
const items = ref<Item[]>([
  {
    id: 101,
    name: '纯棉白T恤',
    category: '1788770285364', // 对应你刚刚新增的上衣T恤ID（模拟）
    location: '主卧衣柜顶部',
    quantity: 3,
    imageUrl: 'https://shadow.elemecdn.com/app/element/hamburger.9cf7b091-55e9-11e9-a976-7f4d0b07eef6.png',
    isPublic: true,
    createdAt: '',
    updatedAt: ''
  },
  {
    id: 102,
    name: '感冒灵颗粒',
    category: '2',
    location: '客厅茶几抽屉',
    quantity: 2,
    isPublic: true,
    createdAt: '',
    updatedAt: ''
  },
  {
    id: 103,
    name: '房产证',
    category: '4',
    location: '书房书架二层',
    quantity: 1,
    isPublic: false, // 游客不可见的隐私物品
    createdAt: '',
    updatedAt: ''
  }
])

// 核心：根据【分类 + 位置 + 关键词 + 用户权限】联动过滤物品
const filteredItems = computed(() => {
  return items.value.filter(item => {
    // 1. 权限过滤：如果是游客，隐藏 isPublic 为 false 的物品
    if (!userStore.isAdmin && !item.isPublic) {
      return false
    }

    // 2. 分类过滤
    const matchesCategory = props.categoryId === 'all' || item.category === props.categoryId

    // 3. 位置过滤
    const matchesLocation = props.location === 'all' || item.location === props.location

    // 4. 搜索框关键词过滤
    const matchesSearch = item.name.toLowerCase().includes(searchQuery.value.toLowerCase())

    return matchesCategory && matchesLocation && matchesSearch
  })
})

const handleDelete = (id: number) => {
  ElMessageBox.confirm('确定删除该物品记录吗？', '警告', {
    confirmButtonText: '删除',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    items.value = items.value.filter(i => i.id !== id)
    ElMessage.success('物品已删除')
  }).catch(() => {})
}
</script>

<style scoped>
.item-list-container {
  padding: 20px;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.search-input {
  width: 300px;
}

.grid-container {
  display: grid;
  /* 响应式网格：卡片最小宽度 240px，多余空间自动撑开 */
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 20px;
}

.item-card {
  border-radius: 8px;
  overflow: hidden;
  transition: transform 0.2s;
}

.item-card:hover {
  transform: translateY(-4px);
}

.image-wrapper {
  position: relative;
  height: 160px;
  background-color: #f8f9fa;
}

.item-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.private-tag {
  position: absolute;
  top: 8px;
  right: 8px;
}

.item-info {
  padding: 12px 15px;
}

.item-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.item-name {
  margin: 0;
  font-size: 16px;
  color: #303133;
}

.item-location {
  font-size: 13px;
  color: #909399;
  margin: 6px 0 12px 0;
}

.item-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
  color: #606266;
}
</style>