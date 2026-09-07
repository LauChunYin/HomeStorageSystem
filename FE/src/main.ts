import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css' // 引入 Element Plus 的默认皮肤样式
import * as ElementPlusIconsVue from '@element-plus/icons-vue' // 引入图标库
import App from './App.vue'

const app = createApp(App) // 1. 创建 Vue 应用实例

// 2. 循环遍历所有 Element Plus 图标并注册到全局，方便随处使用
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(createPinia())    // 3. 安装 Pinia 状态管理插件
app.use(ElementPlus)     // 4. 安装 Element Plus UI 组件库
app.mount('#app')        // 5. 将应用挂载到 index.html 的 #app 节点上，渲染到页面