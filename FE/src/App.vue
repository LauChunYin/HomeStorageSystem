<template>
  <div class="app-container">
    <Navbar />
    <div class="main-layout">
      <!-- 左侧分类侧边栏 -->
      <CategorySidebar @select-category="handleCategorySelect" />

      <!-- 右侧主体内容 -->
      <div class="content-area">
        <!-- 顶部位置筛选 bar -->
        <LocationFilter @select-location="handleLocationSelect" />

        <!-- 物品网格卡片区 -->
        <ItemList 
          :category-id="currentCategory" 
          :location="currentLocation" 
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Navbar from './components/Navbar.vue'
import CategorySidebar from './components/CategorySidebar.vue'
import LocationFilter from './components/LocationFilter.vue'
import ItemList from './components/ItemList.vue'

const currentCategory = ref('all')
const currentLocation = ref('all')

const handleCategorySelect = (categoryId: string) => {
  currentCategory.value = categoryId
}

const handleLocationSelect = (location: string) => {
  currentLocation.value = location
}
</script>

<style>
body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  background-color: #f5f7fa;
  overflow: hidden;
}

.app-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-layout {
  display: flex;
  flex: 1;
  height: calc(100vh - 60px);
}

.content-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}
</style>