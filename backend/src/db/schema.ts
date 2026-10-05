import { sqliteTable, text, integer, real } from 'drizzle-orm/sqlite-core';
import { sql } from 'drizzle-orm';

export const users = sqliteTable('users', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  email: text('email').notNull().unique(),
  phone: text('phone').unique(),
  passwordHash: text('password_hash').notNull(),
  fullName: text('full_name'),
  role: text('role').notNull(), // Admin, Supplier, etc.
  status: text('status').default('Active'),
  createdDate: integer('created_date', { mode: 'timestamp' }).default(sql`(strftime('%s', 'now'))`)
});

export const suppliers = sqliteTable('suppliers', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  userId: integer('user_id').notNull().references(() => users.id).unique(),
  warehouseName: text('warehouse_name'),
  address: text('address'),
  latitude: real('latitude'),
  longitude: real('longitude'),
  createdDate: integer('created_date', { mode: 'timestamp' }).default(sql`(strftime('%s', 'now'))`)
});

export const materialBatches = sqliteTable('material_batch', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  batchCode: text('batch_code').notNull().unique(),
  supplierId: integer('supplier_id').notNull().references(() => suppliers.id),
  weightKg: real('weight_kg'),
  collectionAddress: text('collection_address'),
  latitude: real('latitude'),
  longitude: real('longitude'),
  collectionTime: integer('collection_time', { mode: 'timestamp' }),
  approvedTime: integer('approved_time', { mode: 'timestamp' }),
  status: text('status').default('Pending'),
  createdDate: integer('created_date', { mode: 'timestamp' }).default(sql`(strftime('%s', 'now'))`)
});

export const materialImages = sqliteTable('material_image', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  batchId: integer('batch_id').notNull().references(() => materialBatches.id),
  imageUrl: text('image_url').notNull(),
  uploadTime: integer('upload_time', { mode: 'timestamp' }).default(sql`(strftime('%s', 'now'))`)
});

export const products = sqliteTable('products', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  batchId: integer('batch_id').notNull().references(() => materialBatches.id),
  productName: text('product_name').notNull(),
  description: text('description'),
  qrToken: text('qr_token').notNull().unique(), // We can generate a uuid in TS
  currentStatus: text('current_status').default('Available'),
  createdDate: integer('created_date', { mode: 'timestamp' }).default(sql`(strftime('%s', 'now'))`)
});

export const productTimelines = sqliteTable('product_timeline', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  productId: integer('product_id').notNull().references(() => products.id),
  timelineTime: integer('timeline_time', { mode: 'timestamp' }),
  location: text('location'),
  latitude: real('latitude'),
  longitude: real('longitude'),
  title: text('title'),
  description: text('description'),
  imageUrl: text('image_url'),
  videoUrl: text('video_url')
});

export const stories = sqliteTable('stories', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  productId: integer('product_id').notNull().references(() => products.id),
  title: text('title').notNull()
});

export const transportationHistories = sqliteTable('transportation_history', {
  id: integer('id').primaryKey({ autoIncrement: true }),
  batchId: integer('batch_id').notNull().references(() => materialBatches.id),
  status: text('status'),
  description: text('description'),
  updateTime: integer('update_time', { mode: 'timestamp' })
});
