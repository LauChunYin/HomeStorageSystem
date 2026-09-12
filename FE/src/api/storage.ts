import request from './request'
import type { Item, Category } from '../types/item'

// --- 分类接口 ---
export const getCategoriesApi = (): Promise<Category[]> => {
  return request.get('/categories')
}

export const createCategoryApi = (data: { name: string; isPublic: boolean }): Promise<Category> => {
  return request.post('/categories', data)
}

// --- 物品接口 ---
export const getItemsApi = (params?: { categoryId?: string; location?: string }): Promise<Item[]> => {
  return request.get('/items', { params })
}

export const createItemApi = (data: Partial<Item>): Promise<Item> => {
  return request.post('/items', data)
}

export const deleteItemApi = (id: number): Promise<void> => {
  return request.delete(`/items/${id}`)
}