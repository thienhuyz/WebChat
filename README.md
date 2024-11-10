# SignalR-Chat

## Database
![image](https://github.com/user-attachments/assets/aed0e7f4-1d48-4d65-b284-6d96100b8fa8)

## 🎯 Mô Tả Đề Tài
Đề tài này nhằm xây dựng một hệ thống chat trực tuyến, cho phép người dùng tạo và tham gia các phòng chat riêng biệt để trao đổi thông tin theo nhóm hoặc cá nhân. Hệ thống sẽ cung cấp các tính năng cơ bản của một ứng dụng chat hiện đại, bao gồm khả năng tạo phòng mới, tham gia và rời khỏi phòng, gửi và nhận tin nhắn trong thời gian thực.

## 📋 Các Tính Năng Chính

| Tính Năng                                | Mô Tả                                                                                                                                             |
|------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| **Tạo và quản lý phòng chat**            | Người dùng có thể tạo phòng chat với tên tùy chỉnh, tham gia hoặc rời khỏi các phòng đã tạo.                                                       |
| **Xác thực người dùng bằng IdentityServer4** | Đảm bảo chỉ người dùng đã đăng nhập mới có thể truy cập hệ thống chat.                                      |
| **Giao diện tương tác**                  | Sử dụng KnockoutJS để tự động cập nhật danh sách tin nhắn và người dùng, kết hợp với JavaScript thuần để xử lý các sự kiện cơ bản trên giao diện.  |
| **Tham gia và rời khỏi phòng chat**      | Người dùng có thể tham gia các phòng chat hoặc rời khỏi khi không muốn tiếp tục tham gia nữa.                                                     |
| **Gửi và nhận tin nhắn thời gian thực**  | Sử dụng SignalR để tin nhắn được cập nhật ngay lập tức trong phòng chat mà không cần tải lại trang.                                                |
| **Hiển thị thông báo sự kiện**           | Hiển thị thông báo khi có người mới tham gia, rời phòng hoặc có tin nhắn mới.                                                                      |

## 🔧 Công Nghệ Sử Dụng

| Công Nghệ      | Mô Tả                                                                                               |
|----------------|-----------------------------------------------------------------------------------------------------|
| **Front-end**  | KnockoutJS, JavaScript thuần, HTML, CSS, Bootstrap để xây dựng giao diện và xử lý các tương tác cơ bản. |
| **Back-end**   | ASP.NET Core cho logic nghiệp vụ, IdentityServer4 cho xác thực người dùng, SignalR cho thời gian thực.  |
| **Cơ sở dữ liệu** | SQL Server để lưu trữ thông tin người dùng, phòng chat và tin nhắn.                                |



