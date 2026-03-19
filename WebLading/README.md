# WebLading

Web nhỏ cho phép **đăng nhập** và **xem (read-only)** dữ liệu nhân vật trong game: username, HP, mana, level, coin, stats...

## Chạy project

Trong thư mục `WebLading`:

```bash
npm install
npm run dev
```

Mở trình duyệt tại `http://localhost:3000`.

## Dùng MongoDB thật (đọc trực tiếp từ game)

Web có thể đọc thẳng MongoDB Atlas giống Unity. Tạo file `.env` trong `WebLading/` dựa theo `.env.example` và điền:

- `MONGODB_URI`: connection string Atlas
- `MONGODB_DB`: mặc định `GameDB`

Collections web sẽ đọc:

- `Users` (field `Username`, `Password`)
- `PlayerProgress` (field `UserId` là string ObjectId)

## Tài khoản mẫu

- Username: `demo`
- Password: `demo123`

## Dữ liệu user

File: `data/users.json`

- `passwordHash` dùng bcrypt.
- Bạn có thể thay nội dung `level/hp/mp/stats...` bằng dữ liệu thật từ game (web hiện chỉ hiển thị, không có API update).

