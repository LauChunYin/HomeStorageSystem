<template>
  <div class="location-filter">
    <span class="label">存储位置：</span>
    <el-radio-group v-model="selectedLocation" size="default" @change="handleChange">
      <el-radio-button value="all">全部位置</el-radio-button>
      <el-radio-button v-for="loc in locations" :key="loc" :value="loc">
        {{ loc }}
      </el-radio-button>
    </el-radio-group>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const emit = defineEmits(['select-location'])
const selectedLocation = ref('all')

// 模拟位置数据
const locations = ref(['客厅茶几抽屉', '厨房双门冰箱', '主卧衣柜顶部', '书房书架二层'])

const handleChange = (val: string | number | boolean | undefined) => {
  emit('select-location', val)
}
</script>

<style scoped>
.location-filter {
  background-color: #fff;
  padding: 12px 20px;
  border-bottom: 1px solid #e6e6e6;
  display: flex;
  align-items: center;
}

.label {
  font-size: 14px;
  color: #606266;
  font-weight: 500;
  margin-right: 12px;
}
</style>