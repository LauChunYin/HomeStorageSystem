// 物品实体接口
export interface Item {
  id: number;
  name: string;
  category: string;
  location?: string;
  locationImageUrl?: string; // 位置图片（如客厅柜子外观图）
  quantity: number;
  imageUrl?: string;
  isPublic: boolean;          // true: 游客可见, false: 仅管理员可见
  createdAt: string;
  updatedAt: string;
}

// 类别接口
export interface Category {
  id: number;
  name: string;
  isPublic: boolean;          // 该类别是否开放给游客
}